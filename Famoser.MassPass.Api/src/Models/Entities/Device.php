<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23/05/2016
 * Time: 21:14
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\EntityBase;

class Device extends EntityBase
{
    public $user_id;
    public $guid;
    public $name;
    public $has_access;
    public $access_revoced_reason;

    public function getTableName()
    {
        return "devices";
    }
}