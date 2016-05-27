<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24/05/2016
 * Time: 21:52
 */

namespace Famoser\MassPass\Middleware;


use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Types\ApiErrorTypes;
use Slim\Http\Environment;
use Slim\Http\Request;
use Slim\Http\Response;
use Slim\Http\Stream;

class ApiVersionMiddleware extends BaseMiddleware
{
    
    public function __invoke(Request $request, Response $response, $next)
    {
        if (strpos($_SERVER["REQUEST_URI"], "/1.0") === 0) {
            $this->container->get('api_settings')["api_version"] = 1;
            $_SERVER["REQUEST_URI"] = str_replace("/1.0", "", $_SERVER["REQUEST_URI"]);
            $newEnviroment = Environment::mock($_SERVER);
            $request = Request::createFromEnvironment($newEnviroment);
            $response = $next($request, $response);
        } else {
            $response->withStatus(406, "API version not supported");
            $resp = new ApiResponse(false, ApiErrorTypes::ApiVersionInvalid);
            $response->withJson($resp);
        }
        return $response;
    }
}