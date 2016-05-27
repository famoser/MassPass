<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 27.05.2016
 * Time: 14:23
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Types\ApiErrorTypes;
use Slim\Http\Response;

class BaseController
{
    const DATABASE_FAILURE = 1;

    protected function returnFailure($failureCode, Response $response)
    {
        if ($failureCode == BaseController::DATABASE_FAILURE)
            return $response->withStatus(ApiErrorTypes::ServerFailure, "Database failure");

        return $response->withStatus(ApiErrorTypes::ServerFailure, "unspecified failure");
    }
}