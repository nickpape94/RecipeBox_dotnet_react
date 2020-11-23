import React from 'react';
import { Link } from 'react-router-dom';
import Moment from 'react-moment';
import moment from 'moment';
import Ratings from '../post/Ratings';
import { deleteFavourite } from '../../actions/favourite';

const ProfileFavouriteItem = ({ favourite, deleteFavourite, user }) => {
	const todaysDate = new Date().toISOString().slice(0, 10).replace(/-/g, '');

	return (
		<tbody>
			<tr>
				<td>{favourite.nameOfDish}</td>
				<td className='hide-sm'>{favourite.cuisine}</td>
				<td className='hide-sm'>
					{/* {todaysDate === moment(favourite.created).format('YYYYMMDD') ? (
						<p>Today</p>
					) : (
						<Moment format='DD/MM/YYYY'>{favourite.created}</Moment>
					)} */}
					{favourite.author === null ? (
						<p>Null</p>
					) : (
						<Link to={`/users/${favourite.userId}`}>{favourite.author}</Link>
					)}
				</td>
				<td className='hide-sm'>
					<Ratings averageRating={favourite.averageRating} ratings={favourite.ratings} />
				</td>
				<td>
					<Link to={`/posts/${favourite.postId}`} className='btn btn-primary'>
						View
					</Link>
					{/* <button className='btn btn-primary'>View</button> */}
					<button className='btn btn-danger' onClick={() => deleteFavourite(user.id, favourite.postId)}>
						<i className='fas fa-trash-alt' />
					</button>
				</td>
			</tr>
		</tbody>
	);
};

export default ProfileFavouriteItem;
