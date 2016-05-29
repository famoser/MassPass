<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 27.05.2016
 * Time: 14:23
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Helpers\DatabaseHelper;
use Famoser\MassPass\Models\Entities\Device;
use Famoser\MassPass\Models\Entities\User;
use Famoser\MassPass\Models\Request\Base\ApiRequest;
use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Types\ApiErrorTypes;
use Interop\Container\ContainerInterface;
use Slim\Http\Response;

class BaseController
{
    protected $container;

    //Constructor
    public function __construct(ContainerInterface $ci)
    {
        $this->container = $ci;
    }

    const DATABASE_FAILURE = 1;

    protected function returnFailure($failureCode, Response $response)
    {
        if ($failureCode == BaseController::DATABASE_FAILURE) {
            return $response->withStatus(ApiErrorTypes::ServerFailure, "Database failure");
        }

        return $response->withStatus(ApiErrorTypes::ServerFailure, "unspecified failure");
    }

    protected function isAuthorized(ApiRequest $request)
    {
        $user = $this->getAuthorizedUser($request);
        if ($user == null)
            return false;
        $device = $this->getAuthorizedDevice($request);
        if ($device != null)
            return $device->has_access;
        return false;
    }

    protected function isWellDefined(ApiRequest $request, $neededProps, $neededArrays = null)
    {
        if ($neededProps != null)
            foreach ($neededProps as $neededProp) {
                if ($request->$neededProp == null)
                    return false;
            }
        if ($neededArrays != null)
            foreach ($neededArrays as $neededArray) {
                if (!is_array($request->$neededArray))
                    return false;
            }
        return true;
    }

    protected function returnAuthorizationFailed(ApiResponse $response)
    {
        $response->ApiError = ApiErrorTypes::NotAuthorized;
        $response->Successfull = false;
        return $response;
    }

    private $authorizedUser;

    /**
     * @param ApiRequest $request
     * @return User
     */
    protected function getAuthorizedUser(ApiRequest $request)
    {
        if ($this->authorizedUser == null) {
            if ($request->UserId != null) {
                $helper = $this->getDatabaseHelper();
                $this->authorizedUser = $helper->getSingleFromDatabase(new User(), "guid=:guid", array("guid" => $request->UserId));
            }
        }
        return $this->authorizedUser;
    }

    private $authorizedDevice;

    /**
     * @param ApiRequest $request
     * @return Device
     */
    protected function getAuthorizedDevice(ApiRequest $request)
    {
        if ($this->authorizedDevice == null) {
            if ($request->DeviceId != null) {
                $authorizedUser = $this->getAuthorizedUser($request);

                if ($authorizedUser != null) {
                    $helper = $this->getDatabaseHelper();
                    $this->authorizedDevice = $helper->getSingleFromDatabase(new Device(), "guid=:guid AND user_id=:user_id", array("guid" => $request->DeviceId, "user_id" => $authorizedUser->id));

                    if ($this->authorizedDevice != null) {
                        $this->authorizedDevice->last_request_date_time = time();
                        $helper->saveToDatabase($this->authorizedDevice);
                    }
                }
            }
        }
        return $this->authorizedDevice;
    }

    private $databaseHelper;

    protected function getDatabaseHelper()
    {
        if ($this->databaseHelper == null)
            $this->databaseHelper = new DatabaseHelper($this->container);
        return $this->databaseHelper;
    }
}