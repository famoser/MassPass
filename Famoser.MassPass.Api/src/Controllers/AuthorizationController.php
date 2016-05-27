<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24/05/2016
 * Time: 21:33
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Helpers\DatabaseHelper;
use Famoser\MassPass\Helpers\RequestHelper;
use Famoser\MassPass\Helpers\ResponseHelper;
use Famoser\MassPass\Models\Entities\AuthorizationCode;
use Famoser\MassPass\Models\Entities\Device;
use Famoser\MassPass\Models\Entities\User;
use Famoser\MassPass\Models\Response\Authorization\AuthorizationResponse;
use Famoser\MassPass\Types\ApiErrorTypes;
use Interop\Container\ContainerInterface;
use Slim\Http\Response;

class AuthorizationController extends BaseController
{
    protected $ci;

    //Constructor
    public function __construct(ContainerInterface $ci)
    {
        $this->ci = $ci;
    }

    public function status($request, $response, $args)
    {
        //your code
        //to access items in the container... $this->ci->get('');
    }

    public function authorize($request, Response $response, $args)
    {
        $model = RequestHelper::parseAuthorisationRequest($request);
        $helper = new DatabaseHelper($this->ci);
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
                return $response->withStatus(ApiErrorTypes::AuthorizationCodeInvalid, "Authorization code not found");
            }
            if ($authCode->valid_from > time() || $authCode->valid_till < time()) {
                return $response->withStatus(ApiErrorTypes::AuthorizationCodeInvalid, "Authorization code not valid");
            }
            //successful! delete auth code now
            $helper->deleteFromDatabase($authCode);
        }

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
        
        $resp = new AuthorizationResponse();
        $resp->Message = "welcome aboard!";

        return ResponseHelper::getJsonResponse($response, $resp);
    }

    public function createauthorization($request, $response, $args)
    {
        //your code
        //to access items in the container... $this->ci->get('');
    }

    public function unauthorize($request, $response, $args)
    {
        //your code
        //to access items in the container... $this->ci->get('');
    }

    public function authorizeddevices($request, $response, $args)
    {
        //your code
        //to access items in the container... $this->ci->get('');
    }
}