import { useParamsStore } from '@/hooks/useParamsStore';
import { Button } from 'flowbite-react';
import React from 'react';
import { AiOutlineClockCircle, AiOutlineSortAscending } from 'react-icons/ai';
import { BsFillStopCircleFill, BsStopwatchFill } from 'react-icons/bs';
import { GiFinishLine, GiFlame } from 'react-icons/gi';
import { IoSpeedometer } from 'react-icons/io5';

const pageSizeButtons = [4, 8, 12];

const orderButtons = [
    {
        label: 'Alphabetical',
        icon: AiOutlineSortAscending,
        value: 'make',
    },
    {
        label: 'End Date',
        icon: AiOutlineClockCircle,
        value: 'endingSoon',
    },
    {
        label: 'Recently Added',
        icon: BsFillStopCircleFill,
        value: 'new',
    },
    {
        label: 'Milage',
        icon: IoSpeedometer,
        value: 'mileage'
    }
];

const filterButtons = [
    {
        label: 'Live Auctions',
        icon: GiFlame,
        value: 'live',
    },
    {
        label: 'Ending < 6 hours',
        icon: GiFinishLine,
        value: 'endingSoon',
    },
    {
        label: 'Completed',
        icon: BsStopwatchFill,
        value: 'finished',
    },
];

export function Filters() {

    const {
        pageSize, 
        setParams, 
        orderBy,
        filterBy,
    } = useParamsStore(state => state);



    return (
        <div className='flex justify-between items-center mb-4'>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Filter By</span>
                <Button.Group>
                    {filterButtons.map(({label, icon: Icon, value}) => (
                        <Button
                            className='focus:ring-0'
                            key={value}
                            onClick={() => setParams({filterBy: value})}
                            color={`${filterBy === value ? 'red' : 'gray'}`}
                        >
                            <Icon 
                                className='mr-3 h-4 w-4'
                            />
                            {label}
                        </Button>
                    ))}
                </Button.Group>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Order By</span>
                <Button.Group>
                    {orderButtons.map(({label, icon: Icon, value}) => (
                        <Button
                            className='focus:ring-0'
                            key={value}
                            onClick={() => setParams({orderBy: value})}
                            color={`${orderBy === value ? 'red' : 'gray'}`}

                        >
                            <Icon 
                                className='mr-3 h-4 w-4'
                            />
                            {label}
                        </Button>
                    ))}
                </Button.Group>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Page Size</span>
                <Button.Group>
                    {pageSizeButtons.map((value, idx) => (
                        <Button 
                            key={idx}
                            onClick={() => setParams({pageSize: value})}
                            color={`${pageSize === value ? 'red' : 'gray'}`}
                            className='focus:ring-0'
                        >
                            {value}
                        </Button>
                    ))}
                </Button.Group>
            </div>
        </div>
    );
};
