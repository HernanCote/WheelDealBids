export type PagedResult<T> = {
    results: T[],
    pageCount: number,
    currentPage: number,
    totalCount: number,
};

export type Auction = {
    id: string,
    reservePrice: number,
    seller: string,
    winner?: string,
    soldAmount: number,
    currentHighBid: number,
    auctionEnd: string,
    status: string,
    make: string,
    model: string,
    year: number,
    color: string,
    mileage: number,
    imageUrl: string,
    createdAt: string,
    updatedAt: string,
};
  