<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 22/05/2016
 * Time: 23:38
 */

namespace Famoser\MassPass\Helpers;


use \Psr\Http\Message\ResponseInterface as Response;

class ResponseHelper
{
    public static function getJsonResponse(Response $response, $model)
    {
        $response->getBody()->write(json_encode($model));
        return $response->withHeader('Content-Type','application/json');
    }
}