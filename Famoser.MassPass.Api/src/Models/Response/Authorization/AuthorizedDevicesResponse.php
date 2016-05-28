<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 09:42
 */

namespace Famoser\MassPass\Models\Response\Authorization;


use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Models\Response\Entities\AuthorizedDeviceEntity;

class AuthorizedDevicesResponse extends ApiResponse
{
    /**
     * @var \Famoser\MassPass\Models\Response\Entities\AuthorizedDeviceEntity
     */
    public $AuthorizedDeviceEntities;
}