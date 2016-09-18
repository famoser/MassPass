<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 28/05/2016
 * Time: 17:41
 */

namespace Famoser\MassPass\Models\Request;


use Famoser\MassPass\Models\Request\Base\ApiRequest;

class UpdateRequest extends ApiRequest
{
    public $ContentId;
    public $VersionId;
}