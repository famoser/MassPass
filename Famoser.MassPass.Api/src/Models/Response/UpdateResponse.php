<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 10:08
 */

namespace Famoser\MassPass\Models\Response;


use Famoser\MassPass\Models\Response\Base\ApiResponse;

class UpdateResponse extends ApiResponse
{
    public $ServerId;
    public $ServerRelationId;
    public $VersionId;
}