'use client';
import { useParamsStore } from '@/hooks/useParamsStore';
import React from 'react'
import { AiFillCar } from 'react-icons/ai';


export default function Logo() {

    const reset = useParamsStore(state => state.reset);
    return (
        <div onClick={reset} className="cursor-pointer flex items-center gap-2 md:text-3xl md:visible font-semibold text-red-500">
            <AiFillCar size={34}/>
            <div className='hidden md:inline-block'>Wheel Deal Bids</div>
        </div>
    );
};
