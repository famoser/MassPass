<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23/05/2016
 * Time: 21:13
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\EntityBase;

class User extends EntityBase
{
    public $guid;
    public $name;

    public function getTableName()
    {
        return "users";
    }
}