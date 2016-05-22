<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 22/05/2016
 * Time: 22:40
 */

use Famoser\MassPass\Helpers\RequestHelper;
use Famoser\MassPass\Helpers\ResponseHelper;
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
    ],
];
$c = new Container($configuration);
$app = new App($c);
$app->post('/authorization', function (Request $request, Response $response) {
    $model = RequestHelper::parseAuthorisationRequest($request);
    ResponseHelper::getJsonResponse($response, $model);
});
$app->post('/refresh', function (Request $request, Response $response) {
    $model = RequestHelper::parseRefreshRequest($request);
    ResponseHelper::getJsonResponse($response, $model);
});
$app->run();