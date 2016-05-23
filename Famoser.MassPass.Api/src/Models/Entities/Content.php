<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23/05/2016
 * Time: 21:17
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\EntityBase;

class Content extends EntityBase
{
    public $user_id;
    public $guid;
    public $relation_guid;
    public $version_id;
    public $creation_time;

    public function getTableName()
    {
        return "contents";
    }
}