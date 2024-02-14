'use client';

import React from 'react'
import Countdown, { CountdownRenderProps, zeroPad } from 'react-countdown';

type Props = {
    auctionEnd: string,
};

const renderer = ({ days, hours, minutes, seconds, completed }: CountdownRenderProps) => {
    return (
        <div className={`border-2 border-white text-white py-1 px-2 rounded-lg flex justify-center 
        ${completed ? 'bg-red-600' : (days === 0 && hours < 10) ? 'bg-amber-600' : 'bg-green-600'}`}>
            {completed 
                ? <span>Auction Finished</span> 
                : <span>{zeroPad(days)}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}</span>}
        </div>
    );
};

export const CountdownTimer = ({ auctionEnd }: Props) => (
    <div>
        <Countdown date={auctionEnd} renderer={renderer} />
    </div>
);
