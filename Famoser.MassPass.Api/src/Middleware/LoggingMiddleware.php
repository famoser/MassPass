<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 28/05/2016
 * Time: 21:49
 */

namespace Famoser\MassPass\Middleware;


use Famoser\MassPass\Helpers\LogHelper;
use Slim\Http\Request;
use Slim\Http\Response;

class LoggingMiddleware extends BaseMiddleware
{
    public function __invoke(Request $request, Response $response, $next)
    {
        LogHelper::configure($this->container->get("settings")["log_path"]);
        $response = $next($request, $response);
        return $response;
    }
}