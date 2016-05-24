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
use Famoser\MassPass\Middleware\ApiVersionMiddlename;
use Famoser\MassPass\Middleware\AuthorizationMiddleware;
use Famoser\MassPass\Middleware\TestsMiddleware;
use Famoser\MassPass\Models\Request\Base\ApiRequest;
use Famoser\MassPass\Models\Request\RefreshRequest;
use \Psr\Http\Message\ServerRequestInterface as Request;
use \Psr\Http\Message\ResponseInterface as Response;
use Slim\App;
use Slim\Container;

require '../../vendor/autoload.php';

$configuration = [
    'settings' => [
        'displayErrorDetails' => true,
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

$app = new App($c);
$app->add(new ApiVersionMiddlename());
$app->add(new TestsMiddleware());
$app->add(new AuthorizationMiddleware());

//echo json_encode($c->get('api_settings'));


$app->group("/authorization", function () {
    $this->post('/authorize', function (Request $request, Response $response) {
        $model = RequestHelper::parseAuthorisationRequest($request);
        //stuff
        ResponseHelper::getJsonResponse($response, $model);
    });
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