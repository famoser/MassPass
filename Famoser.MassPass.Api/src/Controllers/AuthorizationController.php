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
use Slim\Http\Request;
use Slim\Http\Response;

class AuthorizationController extends BaseController
{
    public function authorize(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseAuthorisationRequest($request);
        if (!$this->isWellDefined($model, array("DeviceName", "UserName")))
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));
        
        $helper = $this->getDatabaseHelper();
        $user = $helper->getSingleFromDatabase(new User(), "guid=:guid", array("guid" => $model->UserId));
        if ($user == null) {
            //create new user & device
            $newUser = new User();
            $newUser->guid = $model->UserId;
            $newUser->name = $model->UserName;
            if (!$helper->saveToDatabase($newUser)) {
                return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
            }

            $user = $newUser;
        } else {
            //authorize device with auth code
            $authCode = $helper->getSingleFromDatabase(new AuthorizationCode(), "user_id=:user_id AND code=:code", array("user_id" => $user->id, "code" => $model->AuthorisationCode));
            if ($authCode == null) {
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::AuthorizationCodeInvalid));
            }
            if ($authCode->valid_from > time() || $authCode->valid_till < time()) {
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::AuthorizationCodeInvalid));
            }
            //successful! delete auth code now
            $helper->deleteFromDatabase($authCode);
        }
        //check if device in database, if so delete it
        $oldDevice = $helper->getSingleFromDatabase(new Device(), "user_id=:user_id AND guid=:guid", array("user_id" => $user->id, "guid" => $model->DeviceId));
        if ($oldDevice == null) {
            $newDevice = new Device();
            $newDevice->user_id = $user->id;
            $newDevice->guid = $model->DeviceId;
            $newDevice->name = $model->DeviceName;
            $newDevice->has_access = true;
            $newDevice->authorization_date_time = time();
            $newDevice->last_modification_date_time = time();
            if (!$helper->saveToDatabase($newDevice)) {
                return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
            }
        } else {
            $oldDevice->name = $model->DeviceName;
            $oldDevice->has_access = true;
            $oldDevice->authorization_date_time = time();
            $oldDevice->last_modification_date_time = time();
            $oldDevice->access_revoked_by_device_id = 0;
            $oldDevice->access_revoked_date_time = 0;
            $oldDevice->access_revoked_reason = null;
            if (!$helper->saveToDatabase($oldDevice)) {
                return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
            }
        }

        $resp = new AuthorizationResponse();
        $resp->Message = "welcome aboard!";

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
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
            }
        }
        return ResponseHelper::getJsonResponse($response, $resp);
    }

    public function createAuthorization(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseCreateAuthorizationRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("AuthorisationCode")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));

            $helper = $this->getDatabaseHelper();
            $req = new AuthorizationCode();
            $req->user_id = $this->getAuthorizedUser($model)->id;
            $req->code = $model->AuthorisationCode;
            $req->valid_from = time();
            $req->valid_till = time() + 60 * 5; //plus 5 min
            if ($helper->saveToDatabase($req)) {
                $resp = new CreateAuthorizationResponse();
                return ResponseHelper::getJsonResponse($response, $resp);
            } else {
                return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
            }
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }

    public function unAuthorize(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseUnAuthorisationRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("DeviceToBlockId")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));

            $helper = $this->getDatabaseHelper();
            $deviceToUnAuthorized = $helper->getSingleFromDatabase(new Device(), "guid=:guid AND user_id=:user_id", array("guid" => $model->DeviceToBlockId, "user_id" => $this->getAuthorizedUser($model)->id));
            if ($deviceToUnAuthorized != null) {
                $deviceToUnAuthorized->has_access = false;
                $deviceToUnAuthorized->access_revoked_reason = $model->Reason;
                $deviceToUnAuthorized->access_revoked_date_time = time();
                $deviceToUnAuthorized->access_revoked_by_device_id = $this->getAuthorizedDevice($model)->id;
                if ($helper->saveToDatabase($deviceToUnAuthorized)) {
                    $resp = new UnAuthorizationResponse();
                    return ResponseHelper::getJsonResponse($response, $resp);
                } else {
                    return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
                }
            } else {
                $resp = new UnAuthorizationResponse(false, ApiErrorTypes::DeviceNotFound);
                return ResponseHelper::getJsonResponse($response, $resp);
            }
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
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
                    $newDev->DeviceId = $authorizedDevice->guid;
                    $newDev->DeviceName = $authorizedDevice->name;
                    $newDev->LastModificationDateTime = FormatHelper::toCSharpDateTime($authorizedDevice->last_modification_date_time);
                    $newDev->LastRequestDateTime = FormatHelper::toCSharpDateTime($authorizedDevice->last_request_date_time);
                    $resp->AuthorizedDeviceEntities[] = $newDev;
                }
                return ResponseHelper::getJsonResponse($response, $resp);

            } else {
                $resp = new UnAuthorizationResponse(false, ApiErrorTypes::DeviceNotFound);
                return ResponseHelper::getJsonResponse($response, $resp);
            }
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }
}