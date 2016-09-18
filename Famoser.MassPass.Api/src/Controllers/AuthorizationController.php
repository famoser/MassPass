<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24/05/2016
 * Time: 21:33
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Helpers\DatabaseHelper;
use Famoser\MassPass\Helpers\FormatHelper;
use Famoser\MassPass\Helpers\RequestHelper;
use Famoser\MassPass\Helpers\ResponseHelper;
use Famoser\MassPass\Models\Entities\AuthorizationCode;
use Famoser\MassPass\Models\Entities\Device;
use Famoser\MassPass\Models\Entities\User;
use Famoser\MassPass\Models\Request\Authorization\AuthorizedDevicesRequest;
use Famoser\MassPass\Models\Response\Authorization\AuthorizationResponse;
use Famoser\MassPass\Models\Response\Authorization\AuthorizationStatusResponse;
use Famoser\MassPass\Models\Response\Authorization\AuthorizedDevicesResponse;
use Famoser\MassPass\Models\Response\Authorization\CreateAuthorizationResponse;
use Famoser\MassPass\Models\Response\Authorization\UnAuthorizationResponse;
use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Models\Response\Entities\AuthorizedDeviceEntity;
use Famoser\MassPass\Types\ApiErrorTypes;
use Interop\Container\ContainerInterface;
use Models\Request\Authorization\WipeUserRequest;
use Models\Response\Authorization\CreateUserResponse;
use Models\Response\Authorization\WipeUserResponse;
use Slim\Http\Request;
use Slim\Http\Response;

class AuthorizationController extends BaseController
{
    public function createUser(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseCreateUserRequest($request);
        if (!$this->isWellDefined($model, array("UserName", "DeviceName")))
            return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

        $helper = $this->getDatabaseHelper();
        $user = $helper->getSingleFromDatabase(new User(), "user_id=:user_id", array("user_id" => $model->UserId));
        if ($user != null) {
            return $this->returnApiError(ApiErrorTypes::UserAlreadyExists, $response);
        }
        //create new user & device
        $newUser = new User();
        $newUser->user_id = $model->UserId;
        $newUser->user_name = $model->UserName;
        if (!$helper->saveToDatabase($newUser)) {
            return $this->returnApiError(ApiErrorTypes::DatabaseFailure, $response);
        }

        $device = new Device();
        $device->authorization_date_time = time();
        $device->has_access = true;
        $device->device_id = $model->DeviceId;
        $device->device_name = $model->DeviceName;
        $device->user_id = $newUser->id;
        $device->last_modification_date_time = time();
        $device->last_request_date_time = time();
        if (!$helper->saveToDatabase($device)) {
            return $this->returnApiError(ApiErrorTypes::DatabaseFailure, $response);
        }

        $resp = new CreateUserResponse();
        $resp->Message = "welcome aboard!";

        return ResponseHelper::getJsonResponse($response, $resp);
    }

    public function authorize(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseAuthorisationRequest($request);
        if (!$this->isWellDefined($model, array("DeviceName")))
            return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

        $helper = $this->getDatabaseHelper();
        $user = $helper->getSingleFromDatabase(new User(), "user_id=:user_id", array("user_id" => $model->UserId));
        if ($user == null) {
            return $this->returnApiError(ApiErrorTypes::UserNotFound, $response);
        }
        //authorize device with auth code
        $authCode = $helper->getSingleFromDatabase(new AuthorizationCode(), "user_id=:user_id AND code=:code", array("user_id" => $user->id, "code" => $model->AuthorisationCode));
        if ($authCode == null) {
            return $this->returnApiError(ApiErrorTypes::AuthorizationCodeInvalid, $response);
        }
        while ($authCode->valid_till < time()) {
            $helper->deleteFromDatabase($authCode);
            return $this->returnApiError(ApiErrorTypes::AuthorizationCodeInvalid, $response);
        }
        //successful! delete auth code now
        if (!$helper->deleteFromDatabase($authCode))
            return $this->returnApiError(ApiErrorTypes::DatabaseFailure, $response);

        //check if device in database, if so update it
        $oldDevice = $helper->getSingleFromDatabase(new Device(), "user_id=:user_id AND device_id=:device_id", array("user_id" => $user->id, "device_id" => $model->DeviceId));
        if ($oldDevice != null) {
            return $this->returnApiError(ApiErrorTypes::DeviceAlreadyExists, $response);
        } else {
            $oldDevice = new Device();
            $oldDevice->device_name = $model->DeviceName;
            $oldDevice->has_access = true;
            $oldDevice->authorization_date_time = time();
            $oldDevice->last_modification_date_time = time();
            $oldDevice->access_revoked_by_device_id = 0;
            $oldDevice->access_revoked_date_time = 0;
            $oldDevice->access_revoked_reason = null;
            if (!$helper->saveToDatabase($oldDevice)) {
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);
            }
        }

        $resp = new AuthorizationResponse();
        $resp->Message = "welcome aboard!";
        $resp->Content = $authCode->content;

        return ResponseHelper::getJsonResponse($response, $resp);
    }

    public function status(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseAuthorizationStatusRequest($request);
        $resp = new AuthorizationStatusResponse();
        if ($this->isAuthorized($model)) {
            $resp->IsAuthorized = true;
        } else {
            $device = $this->getAuthorizedDevice($model);
            if ($device != null) {
                $resp->IsAuthorized = false;
                $resp->UnauthorizedReason = $device->access_revoked_reason;
            } else {
                return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
            }
        }
        return ResponseHelper::getJsonResponse($response, $resp);
    }

    public function wipeUser(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseWipeUserRequest($request);
        if ($this->isAuthorized($model)) {
            $helper = new DatabaseHelper($this->container);
            $devices = $helper->getFromDatabase(new Device(), "user_id=:user_id", array("user_id" => $this->getAuthorizedUser($model)->id));
            foreach ($devices as $device) {
                if (!$helper->deleteFromDatabase($device)) {
                    return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);
                }
            }
            if (!$helper->deleteFromDatabase($this->getAuthorizedUser($model))) {
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);
            }
        }

        $resp = new WipeUserResponse();
        return ResponseHelper::getJsonResponse($response, $resp);
    }

    public function createAuthorization(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseCreateAuthorizationRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("AuthorisationCode")))
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

            $helper = $this->getDatabaseHelper();
            $req = new AuthorizationCode();
            $req->user_id = $this->getAuthorizedUser($model)->id;
            $req->code = $model->AuthorisationCode;
            $req->content = $model->Content;
            $req->valid_till = time() + 60 * 5; //plus 5 min
            if ($helper->saveToDatabase($req)) {
                $resp = new CreateAuthorizationResponse();
                return ResponseHelper::getJsonResponse($response, $resp);
            } else {
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);
            }
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    public function unAuthorize(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseUnAuthorisationRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("DeviceToBlockId")))
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

            $helper = $this->getDatabaseHelper();
            $deviceToUnAuthorized = $helper->getSingleFromDatabase(new Device(), "device_id=:device_id AND user_id=:user_id", array("device_id" => $model->DeviceToBlockId, "user_id" => $this->getAuthorizedUser($model)->id));
            if ($deviceToUnAuthorized != null) {
                $deviceToUnAuthorized->has_access = false;
                $deviceToUnAuthorized->access_revoked_reason = $model->Reason;
                $deviceToUnAuthorized->access_revoked_date_time = time();
                $deviceToUnAuthorized->access_revoked_by_device_id = $this->getAuthorizedDevice($model)->id;
                if ($helper->saveToDatabase($deviceToUnAuthorized)) {
                    $resp = new UnAuthorizationResponse();
                    return ResponseHelper::getJsonResponse($response, $resp);
                } else {
                    return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);
                }
            } else {
                return $this->returnApiError(ApiErrorTypes::DeviceNotFound, $response);
            }
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    public function authorizedDevices(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseAuthorizedDevicesRequest($request);
        if ($this->isAuthorized($model)) {
            $helper = $this->getDatabaseHelper();
            $resp = new AuthorizedDevicesResponse();
            $authorizedDevices = $helper->getFromDatabase(new Device(), "user_id=:user_id", array("user_id" => $this->getAuthorizedUser($model)->id));
            if ($authorizedDevices != null && count($authorizedDevices) > 0) {
                $resp->AuthorizedDeviceEntities = array();
                foreach ($authorizedDevices as $authorizedDevice) {
                    $newDev = new AuthorizedDeviceEntity();
                    $newDev->AuthorizationDateTime = FormatHelper::toCSharpDateTime($authorizedDevice->authorization_date_time);
                    $newDev->DeviceId = $authorizedDevice->user_id;
                    $newDev->DeviceName = $authorizedDevice->user_name;
                    $newDev->LastModificationDateTime = FormatHelper::toCSharpDateTime($authorizedDevice->last_modification_date_time);
                    $newDev->LastRequestDateTime = FormatHelper::toCSharpDateTime($authorizedDevice->last_request_date_time);
                    $resp->AuthorizedDeviceEntities[] = $newDev;
                }
                return ResponseHelper::getJsonResponse($response, $resp);

            } else {
                return $this->returnApiError(ApiErrorTypes::NoDevicesFound, $response);
            }
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }
}