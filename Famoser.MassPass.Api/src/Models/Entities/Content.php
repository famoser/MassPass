<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23/05/2016
 * Time: 21:17
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\BaseEntity;

class Content extends BaseEntity
{
    public $user_id;
    public $guid;
    public $relation_guid;
    public $version_id;

    public function getTableName()
    {
        return "contents";
    }
}