<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24/05/2016
 * Time: 21:46
 */

namespace Famoser\MassPass\Middleware;


use Slim\Http\Environment;
use Slim\Http\Request;
use Slim\Http\Response;
use Slim\Http\Uri;

class TestsMiddleware
{
    public function __invoke(Request $request, Response $response, $next)
    {
        if (strpos($_SERVER["REQUEST_URI"], "/tests") === 0) {
            $this->get('api_settings')["test_mode"] = true;
            $_SERVER["REQUEST_URI"] = str_replace("/tests", "", $_SERVER["REQUEST_URI"]);
            $newEnviroment = Environment::mock($_SERVER);
            $request = Request::createFromEnvironment($newEnviroment);
        }
        $response = $next($request, $response);
        return $response;
    }
}