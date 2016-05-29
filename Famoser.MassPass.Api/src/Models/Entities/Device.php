<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23/05/2016
 * Time: 21:14
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\BaseEntity;

class Device extends BaseEntity
{
    public $user_id;
    public $guid;
    public $name;
    public $has_access;
    public $last_modification_date_time;
    public $last_request_date_time;
    public $authorization_date_time;
    public $access_revoked_reason;
    public $access_revoked_by_device_id;
    public $access_revoked_date_time;

    public function getTableName()
    {
        return "devices";
    }
}