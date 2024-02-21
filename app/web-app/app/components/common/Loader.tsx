'use client';
import React from 'react'; 
import { Audio } from 'react-loader-spinner';
  

type LoaderProps = {
  color?: string,
  height?: number,
  width?: number,
  isVisible?: boolean,
};

export function Loader({
  color = '#F05152',
  height = 100,
  width = 100,
  isVisible = true,
}: LoaderProps) { 
  return ( 
    <div className='flex flex-col justify-center items-center'> 
      <Audio 
        visible={isVisible}
        color={color}
        height={height} 
        width={width}   
      /> 
    </div> 
  );
};