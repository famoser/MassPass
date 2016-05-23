<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 10:19
 */

namespace Famoser\MassPass\Middleware;


use \Psr\Http\Message\ServerRequestInterface as Request;
use \Psr\Http\Message\ResponseInterface as Response;


class AuthorizationMiddleware
{
    public function __invoke(Request $request, Response $response, $next)
    {
        $uri = $request->getUri();
        if ($uri->getPath() != "/authorization")
        {
            //check if access granted
            //omitted, doing authentication directly in index, to vermeiden duplicated code
        }
        $response = $next($request, $response);
        return $response;
    }
}