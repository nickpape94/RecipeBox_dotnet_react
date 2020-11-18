import React from 'react';

const Ratings = ({ averageRating, ratings }) => (
	<div className='ratings'>
		{/* {console.log(averageRating)}
		{console.log(ratings.length)} */}
		{averageRating === 0 && (
			<div>
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 0 &&
		averageRating <= 0.5 && (
			<div>
				<span className='fas fa-star-half-alt checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 0.5 &&
		averageRating <= 1 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 1 &&
		averageRating <= 1.5 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fas fa-star-half-alt checked' />
				<span className='far fa-star checked ' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 1.5 &&
		averageRating <= 2 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 2 &&
		averageRating <= 2.5 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fas fa-star-half-alt checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 2.5 &&
		averageRating <= 3 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='far fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 3 &&
		averageRating <= 3.5 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fas fa-star-half-alt checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 3.5 &&
		averageRating <= 4 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='far fa-star checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 4 &&
		averageRating <= 4.5 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fas fa-star-half-alt checked' />
				({ratings.length})
			</div>
		)}
		{averageRating > 4.5 &&
		averageRating <= 5 && (
			<div>
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				<span className='fa fa-star checked' />
				({ratings.length})
			</div>
		)}
	</div>
);

export default Ratings;
