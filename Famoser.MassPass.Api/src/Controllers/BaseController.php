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
        if ($failureCode == BaseController::DATABASE_FAILURE)
            return $response->withStatus(ApiErrorTypes::ServerFailure, "Database failure");

        return $response->withStatus(ApiErrorTypes::ServerFailure, "unspecified failure");
    }

    protected function isAuthorized(ApiRequest $request)
    {
        $user = $this->getAuthorizedUser($request);
        if ($user == null)
            return false;
        $device = $this->getAuthorizedDevice($request, $user);
        if ($device != null)
            return $device->has_access;
        return false;
    }

    private $authorizedUser;

    protected function getAuthorizedUser(ApiRequest $request)
    {
        if ($this->authorizedUser == null) {
            $helper = $this->getDatabaseHelper();
            $this->authorizedUser = $helper->getSingleFromDatabase(new User(), "guid=:guid", array("guid" => $request->UserId));
        }
        return $this->authorizedUser;
    }

    private $authorizedDevice;

    protected function getAuthorizedDevice(ApiRequest $request, User $authorizedUser)
    {
        if ($this->authorizedDevice == null) {
            if ($authorizedUser == null)
                $authorizedUser = $this->getAuthorizedUser($request);

            $helper = $this->getDatabaseHelper();
            $this->authorizedDevice = $helper->getSingleFromDatabase(new Device(), "guid=:guid AND user_id=:user_id", array("guid" => $request->DeviceId, "user_id" => $authorizedUser->id));
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