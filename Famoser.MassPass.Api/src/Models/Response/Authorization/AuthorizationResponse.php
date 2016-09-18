<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 09:40
 */

namespace Famoser\MassPass\Models\Response\Authorization;


use Famoser\MassPass\Models\Response\Base\ApiResponse;

class AuthorizationResponse extends ApiResponse
{
    public $Message;
    public $Content;
}