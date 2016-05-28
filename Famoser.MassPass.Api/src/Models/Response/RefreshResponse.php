<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 09:51
 */

namespace Famoser\MassPass\Models\Response;


use Famoser\MassPass\Models\Response\Base\ApiResponse;

class RefreshResponse extends ApiResponse
{
    /*
     * @var Entities\RefreshEntity[]
     */
    public $RefreshEntities;
}