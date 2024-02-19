'use server'

import { Auction, PagedResult } from '@/types';

export async function getAuctions(query: string) : Promise<PagedResult<Auction>> {
    const uri = `http://localhost:6001/search${query}`;
    const res = await fetch(uri);
  
    if (!res.ok)
      throw new Error('Failed to fetch data');
  
    return res.json();
};