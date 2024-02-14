import React from 'react'
import { AuctionCard } from './AuctionCard';

async function getData(pageSize: number) {
  const res = await fetch(`http://localhost:6001/search?pageSize=${pageSize}`);

  if (!res.ok)
    throw new Error('Failed to fetch data');

  return res.json();
};

async function Listings() {

  const data = await getData(10);

  return (
    <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6'>
      {data && data.results.map((auction: any) => (
        <AuctionCard key={auction.id} {...auction} />
      ))}
    </div>
  );
};

export { Listings };