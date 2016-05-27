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
        'db' => [
            'path' => "sqlite.db",
            'test_path' => "sqlite_tests.db"
        ],
        'data_path' => realpath("../../app")
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
$c['notFoundHandler'] = function ($c) {
    return function ($request, $response) use ($c) {
        $res = new ApiResponse(false, ApiErrorTypes::RequestUriInvalid);
        return $response->withStatus(404, "endpoint not found")->withJson($res);
    };
};
$c['notAllowedHandler'] = $c['notFoundHandler'];
$c['errorHandler'] = function ($c) {
    /**
     * @param $request
     * @param $response
     * @param $exception
     * @return mixed
     */
    return function ($request, Response $response, Exception $exception) use ($c) {
        $res = new ApiResponse(false, ApiErrorTypes::ServerFailure);
        return $response->withStatus(500, $exception->getMessage())->withJson($res);
    };
};


$app = new App($c);
$app->add(new JsonMiddleware());
$app->add(new AuthorizationMiddleware());
$app->add(new ApiVersionMiddleware($c));
$app->add(new TestsMiddleware($c));


$app->group("/authorization", function () {
    $this->post('/authorize', '\AuthorizationController:authorize');
    $this->post('/unauthorize', function (Request $request, Response $response) {
        $model = RequestHelper::parseAuthorisationRequest($request);
        //stuff
        ResponseHelper::getJsonResponse($response, $model);
    });
    $this->post('/authorizeddevices', function (Request $request, Response $response) {
        $model = RequestHelper::parseAuthorisationRequest($request);
        //stuff
        ResponseHelper::getJsonResponse($response, $model);
    });
});
$app->post('/refresh', function (Request $request, Response $response) {
    $model = RequestHelper::parseRefreshRequest($request);
    ResponseHelper::getJsonResponse($response, $model);
});
$app->run();