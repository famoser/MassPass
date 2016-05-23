<?php

namespace Famoser\MassPass\Helpers;
use Famoser\MassPass\Models\Request\Authorization\AuthorizationRequest;
use Famoser\MassPass\Models\Request\Authorization\AuthorizedDevicesRequest;
use Famoser\MassPass\Models\Request\Authorization\UnAuthorizationRequest;
use Famoser\MassPass\Models\Request\CollectionEntriesRequest;
use Famoser\MassPass\Models\Request\ContentEntityHistoryRequest;
use Famoser\MassPass\Models\Request\ContentEntityRequest;
use Famoser\MassPass\Models\Request\RefreshRequest;
use Famoser\MassPass\Models\Response\Authorization\AuthorizedDevices;
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
        RequestHelper::executeJsonMapper($request, new AuthorizationRequest());
    }

    /**
     * @param Request $request
     * @return UnAuthorizationRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseUnAuthorisationRequest(Request $request)
    {
        RequestHelper::executeJsonMapper($request, new UnAuthorizationRequest());
    }

    /**
     * @param Request $request
     * @return AuthorizedDevicesRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseAuthorizedDevicesRequest(Request $request)
    {
        RequestHelper::executeJsonMapper($request, new AuthorizedDevicesRequest());
    }

    /**
     * @param Request $request
     * @return CollectionEntriesRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseCollectionEntriesRequest(Request $request)
    {
        RequestHelper::executeJsonMapper($request, new CollectionEntriesRequest());
    }

    /**
     * @param Request $request
     * @return ContentEntityHistoryRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseContentEntityHistoryRequest(Request $request)
    {
        RequestHelper::executeJsonMapper($request, new ContentEntityHistoryRequest());
    }

    /**
     * @param Request $request
     * @return ContentEntityRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseContentEntityRequest(Request $request)
    {
        RequestHelper::executeJsonMapper($request, new ContentEntityRequest());
    }

    /**
     * @param Request $request
     * @return RefreshRequest
     * @throws \JsonMapper_Exception
     */
    public static function parseRefreshRequest(Request $request)
    {
        RequestHelper::executeJsonMapper($request, new RefreshRequest());
    }

    private static function executeJsonMapper(Request $request, $model)
    {
        $jsonObj = $request->getParsedBody();
        $mapper = new JsonMapper();
        $mapper->bExceptionOnUndefinedProperty = true;
        return $mapper->map($jsonObj, $model);
    }
}