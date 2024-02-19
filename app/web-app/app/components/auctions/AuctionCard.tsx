import React from 'react'

import { CardImage, CountdownTimer } from '../common';

import { Auction } from '@/types';

export function AuctionCard({ 
    make, 
    imageUrl, 
    model, 
    year,
    auctionEnd,
}: Auction) {
  return (
    <a href='#' className="group">
        <div className="w-full bg-gray-200 aspect-w-16 aspect-h-10 rounded-lg overflow-hidden">
            <div>
                <CardImage imageUrl={imageUrl} alt="auction image" />
                <div className="absolute bottom-2 left-2">
                    <CountdownTimer auctionEnd={auctionEnd} />
                </div>
            </div>
        </div>
        <div className="flex justify-between items-center mt-4">
            <h3 className="text-gray-700">{make} {model}</h3>
            <p className="font-semibold text-sm">{year}</p>
        </div>
    </a>
  );
};