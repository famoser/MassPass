<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 28.05.2016
 * Time: 11:14
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Helpers\ResponseHelper;
use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Types\ApiErrorTypes;
use Slim\Http\Request;
use Slim\Http\Response;

class ActionsController extends BaseController
{
    public function cleanup(Request $request, Response $response, $args)
    {
        if ($request->getAttribute("test_mode") == true) {
            $model = new ApiResponse();
            $tempFilePath = $this->container["settings"]["data_path"] . "/.db_created";
            if (file_exists($tempFilePath))
                unlink($tempFilePath);

            return ResponseHelper::getJsonResponse($response, $model);
        } else {
            $model = new ApiResponse(false, ApiErrorTypes::Forbidden);
            return ResponseHelper::getJsonResponse($response, $model);
        }
    }
}