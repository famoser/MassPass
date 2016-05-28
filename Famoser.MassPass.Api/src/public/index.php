<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 22/05/2016
 * Time: 22:40
 */

use Famoser\MassPass\Helpers\DatabaseHelper;
use Famoser\MassPass\Helpers\RequestHelper;
use Famoser\MassPass\Helpers\ResponseHelper;
use Famoser\MassPass\Middleware\ApiVersionMiddleware;
use Famoser\MassPass\Middleware\AuthorizationMiddleware;
use Famoser\MassPass\Middleware\JsonMiddleware;
use Famoser\MassPass\Middleware\LoggingMiddleware;
use Famoser\MassPass\Middleware\TestsMiddleware;
use Famoser\MassPass\Models\Request\Base\ApiRequest;
use Famoser\MassPass\Models\Request\RefreshRequest;
use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Types\ApiErrorTypes;
use \Psr\Http\Message\ServerRequestInterface as Request;
use \Psr\Http\Message\ResponseInterface as Response;
use Slim\App;
use Slim\Container;

require '../../vendor/autoload.php';

$configuration = [
    'settings' => [
        'displayErrorDetails' => false,
        'debug_mode' => true,
        'db' => [
            'path' => "sqlite.db",
            'test_path' => "sqlite_tests.db"
        ],
        'data_path' => realpath("../../app"),
        'asset_path' => realpath("../Assets"),
        'log_path' => realpath("../../app/logs"),
    ],
    'api_settings' => [
        'api_version' => 1,
        'test_mode' => false
    ]
];

$c = new Container($configuration);
$c['db'] = function ($c) {
    $db = $c['settings']['db'];
    $pathKey = $c['api_settings']['test_mode'] ? 'test_path' : 'path';
    $pdo = new PDO("sqlite:" . $c["settings"]["data_path"] . "/" . $db[$pathKey]);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
    return $pdo;
};
$c['notFoundHandler'] = function (Container $c) {
    return function (Request $req, Response $resp) use ($c) {
        $res = new ApiResponse(false, ApiErrorTypes::RequestUriInvalid);
        if ($c->get("settings")["debug_mode"])
            $res->DebugMessage = "requested: " . $req->getRequestTarget();

        return $resp->withStatus(404, "endpoint not found")->withJson($res);
    };
};
$c['notAllowedHandler'] = $c['notFoundHandler'];
$c['errorHandler'] = function (Container $c) {
    /**
     * @param $request
     * @param $response
     * @param $exception
     * @return mixed
     */
    return function (Request $request, Response $response, Exception $exception) use ($c) {
        $res = new ApiResponse(false, ApiErrorTypes::ServerFailure);
        if ($c->get("settings")["debug_mode"])
            $res->DebugMessage = "Exception: " . $exception->getMessage() . " \nStack: " . $exception->getTraceAsString();
        return $response->withStatus(500, $exception->getMessage())->withJson($res);
    };
};

$controllerNamespace = 'Famoser\MassPass\Controllers\\';

$app = new App($c);
$app->add(new JsonMiddleware());
$app->add(new AuthorizationMiddleware());
$app->add(new ApiVersionMiddleware($c));
$app->add(new TestsMiddleware($c));
$app->add(new LoggingMiddleware($c));

$routes = function () use ($controllerNamespace) {
    $this->group("/authorization", function () use ($controllerNamespace) {
        $this->post('/authorize', $controllerNamespace . 'AuthorizationController:authorize');
        $this->post('/status', $controllerNamespace . 'AuthorizationController:status');
        $this->post('/createauthorization', $controllerNamespace . 'AuthorizationController:createAuthorization');
        $this->post('/unauthorize', $controllerNamespace . 'AuthorizationController:unAuthorize');
        $this->post('/authorizeddevices', $controllerNamespace . 'AuthorizationController:authorizedDevices');
    });
    $this->group("/actions", function () use ($controllerNamespace) {
        $this->get('/cleanup', $controllerNamespace . 'ActionsController:cleanup');
        $this->get('/setup', $controllerNamespace . 'ActionsController:setup');
    });
};


$app->group("/tests/1.0", $routes);
$app->group("/1.0", $routes);

$app->run();