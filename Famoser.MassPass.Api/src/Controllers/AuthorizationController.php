<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24/05/2016
 * Time: 21:33
 */

namespace Famoser\MassPass\Controllers;


use Interop\Container\ContainerInterface;

class AuthorizationController
{
    protected $ci;
    //Constructor
    public function __construct(ContainerInterface $ci) {
        $this->ci = $ci;
    }

    public function method1($request, $response, $args) {
        //your code
        //to access items in the container... $this->ci->get('');
    }

    public function method2($request, $response, $args) {
        //your code
        //to access items in the container... $this->ci->get('');
    }

    public function method3($request, $response, $args) {
        //your code
        //to access items in the container... $this->ci->get('');
    }
}