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
        $files = glob($this->container->get("settings")["log_path"] . '/*'); // get all file names
        foreach ($files as $file) { // iterate files
            if (is_file($file))
                unlink($file); // delete file
        }

        $response = $next($request, $response);
        return $response;
    }
}