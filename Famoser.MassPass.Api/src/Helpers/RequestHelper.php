<?php

namespace Famoser\MassPass\Helpers;
use Famoser\MassPass\Models\Request\Authorization\AuthorizationRequest;
use Famoser\MassPass\Models\Request\RefreshRequest;
use JsonMapper;
use \Psr\Http\Message\ServerRequestInterface as Request;

/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 22/05/2016
 * Time: 23:35
 */
class RequestHelper
{
    /**
     * @param Request $request
     * @return AuthorizationRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseAuthorisationRequest(Request $request)
    {
        $json = json_decode($request->getBody());
        $mapper = new JsonMapper();
        $mapper->bExceptionOnUndefinedProperty = true;
        return $mapper->map($json, new AuthorizationRequest());
    }
    /**
     * @param Request $request
     * @return RefreshRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseRefreshRequest(Request $request)
    {
        $json = json_decode($request->getBody());
        $mapper = new JsonMapper();
        $mapper->bExceptionOnUndefinedProperty = true;
        return $mapper->map($json, new RefreshRequest());
    }
}