<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 09:49
 */

namespace Famoser\MassPass\Models\Response;


use Famoser\MassPass\Models\Response\Base\ApiResponse;

class ContentEntityHistoryResponse extends ApiResponse
{
    /*
     * @var Entities\HistoryEntry[]
     */
    public $HistoryEntries;
}