'use client'
import React, { useEffect, useState } from 'react';
import { shallow } from 'zustand/shallow';
import qs from 'query-string';

import { useParamsStore } from '@/hooks/useParamsStore';

import { Filters, Pagination, EmptyFilter, Loader } from '../common';
import { AuctionCard } from './AuctionCard';

import { getAuctions } from '@/app/actions/auctionActions';
import { Auction, PagedResult } from '@/types';


export function Listings() {
  
  const [data, setData] = useState<PagedResult<Auction>>();

  const params = useParamsStore(state => ({
    pageNumber: state.pageNumber,
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy: state.orderBy,
    filterBy: state.filterBy,
  }), shallow);

  const setParams = useParamsStore(state => state.setParams);
  const url = qs.stringifyUrl({url: '', query: params});

  function setPageNumber(pageNumber: number) {
    setParams({ pageNumber });
  };

  useEffect(() => {
    (async () => {
      const auctionsList = await getAuctions(url);
      setData(auctionsList);
    })();
  }, [url]);

  if (!data) {
    return (
      <div className='h-[90vh] flex flex-col justify-center items-center'>
        <Loader />
      </div>
    );
  }
  
  return (
    <>
      <Filters />
      {
        data.totalCount === 0 
          ? <EmptyFilter showReset /> 
          : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {data.results.map(auction => (
              <AuctionCard key={auction.id} {...auction} />
            ))}
            </div>
            <div className="flex justify-center mt-4">
              <Pagination currentPage={params.pageNumber} pageCount={data.pageCount} pageChanged={setPageNumber}/>
            </div>
          </>
        )
      }
    </>
  );
};
