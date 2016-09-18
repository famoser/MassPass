<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 22/05/2016
 * Time: 22:46
 */

namespace Famoser\MassPass\Models\Request\Authorization;


use Famoser\MassPass\Models\Request\Base\ApiRequest;

class AuthorizationRequest extends ApiRequest
{
    public $AuthorisationCode;
    public $DeviceName;
}