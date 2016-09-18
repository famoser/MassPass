<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 09:41
 */

namespace Famoser\MassPass\Models\Response\Base;


use Famoser\MassPass\Types\ApiErrorTypes;

class ApiResponse
{
    public function __construct($successfull = true, $apiError = ApiErrorTypes::None)
    {
        $this->Successfull = $successfull;
        $this->ApiError = $apiError;
    }

    public $Successfull;
    public $ApiError;
    public $ApiMessage;
}