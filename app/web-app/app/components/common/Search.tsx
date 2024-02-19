'use client';

import React, { useEffect, useState } from 'react'
import { FaSearch } from 'react-icons/fa';

import { useParamsStore } from '@/hooks/useParamsStore';

export function Search() {
    
    const {
        setParams, 
        setSearchValue, 
        searchValue,
    } = useParamsStore(state => state);

    function search() {
        setParams({ searchTerm: searchValue });
    };

    function onKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            search();
        }
    };

    return (
        <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
            <input 
                onKeyDown={onKeyDown}
                type="text" 
                onChange={e => setSearchValue(e.target.value)}
                placeholder="Search by make, model or color"
                value={searchValue}
                className='flex-grow pl-5 bg-transparent focus:outline-none border-transparent focus:border-transparent focus:ring-0 text-gray-600'
            />
            <button onClick={search}>
                <FaSearch size={34} className='bg-red-400 text-white p-2 rounded-full cursor-pointer mx-2'/>
            </button>
        </div>
    );
};
