<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 22/05/2016
 * Time: 23:01
 */

namespace Famoser\MassPass\Models\Request;


use Famoser\MassPass\Models\Request\Base\ApiRequest;

class CollectionEntriesRequest extends ApiRequest
{
    public $Guid;
    
    /*
     * @var string[]
     */
    public $KnownServerIds;
}