<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 28/05/2016
 * Time: 17:28
 */

namespace Famoser\MassPass\Models\Request\Authorization;


use Famoser\MassPass\Models\Request\Base\ApiRequest;

class CreateAuthorizationRequest extends ApiRequest
{
    public $AuthorisationCode;
    public $Content;
}