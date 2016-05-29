<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 29/05/2016
 * Time: 20:15
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\BaseEntity;

class ContentHistory extends BaseEntity
{
    public $content_id;
    public $device_id;
    public $version_id;
    public $creation_date_time;
    
    public function getTableName()
    {
        return "content_history";
    }
}