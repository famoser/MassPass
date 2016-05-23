<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23/05/2016
 * Time: 21:16
 */

namespace Famoser\MassPass\Models\Entities;


use Famoser\MassPass\Models\Entities\Base\EntityBase;

class AuthorizationCode extends EntityBase
{
    public $user_id;
    public $content_id;
    public $code;
    public $valid_from;
    public $valid_till;

    public function getTableName()
    {
        return "authorization_codes";
    }
}