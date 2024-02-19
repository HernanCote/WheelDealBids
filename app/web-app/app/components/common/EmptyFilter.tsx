import { useParamsStore } from '@/hooks/useParamsStore';
import React from 'react';
import { Heading } from './Heading';
import { Button } from 'flowbite-react';

type EmptyFilterProps = {
    title?: string,
    subtitle?: string,
    showReset: boolean,
};

export function EmptyFilter({ 
    title = 'No matches for this filter', 
    subtitle = 'Try changing or reseting the filter', 
    showReset,
}: EmptyFilterProps) {
    const reset = useParamsStore(state => state.reset);

    return (
        <div className='h-[40vh] flex flex-col gap-2 justify-center items-center'>
            <Heading title={title} subtitle={subtitle} center />
            {showReset && (
                <div className="mt-4">
                    <Button 
                        outline 
                        onClick={reset}
                    >
                        Remove Filters
                    </Button>
                </div>
            )}
        </div>
    );
};
