<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 28/05/2016
 * Time: 17:44
 */

namespace Famoser\MassPass\Models\Response\Authorization;


use Famoser\MassPass\Models\Response\Base\ApiResponse;

class UnAuthorizationResponse extends ApiResponse
{
    public $Message;
}