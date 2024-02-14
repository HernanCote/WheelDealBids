import Image from 'next/image';
import React from 'react'
import { CountdownTimer } from '.';

type Props = {
    make: string,
    imageUrl: string,
    model: string,
    year: number,
    auctionEnd: string,
};

export function AuctionCard({ 
    make, 
    imageUrl, 
    model, 
    year, 
    auctionEnd 
}: Props) {
  return (
    <a href='#'>
        <div className="w-full bg-gray-200 aspect-w-16 aspect-h-10 rounded-lg overflow-hidden">
            <div>
                <Image 
                    src={imageUrl} 
                    alt="auction image" 
                    fill
                    priority
                    className='object-cover'
                    sizes='(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw'
                />
                <div className='absolute bottom-2 left-2'>
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