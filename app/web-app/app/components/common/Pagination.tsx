'use client'

import { Pagination as FBPagination } from 'flowbite-react';
import React from 'react';

type Props = {
    currentPage: number,
    pageCount: number,
    pageChanged: (page: number) => void,
};

export const Pagination = ({
    currentPage, 
    pageCount, 
    pageChanged,
} : Props) => (
    <FBPagination 
        currentPage={currentPage}
        onPageChange={e => pageChanged(e)}
        totalPages={pageCount}
        layout='pagination'
        showIcons
        className='text-blue-500 mb-5'
    />
);
