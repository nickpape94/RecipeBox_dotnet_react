import React from 'react';
import { Link } from 'react-router-dom';
import Moment from 'react-moment';
import moment from 'moment';
import Ratings from '../post/Ratings';

const ProfileItem = ({ post, fromAuthProfile = false }) => {
	const todaysDate = new Date().toISOString().slice(0, 10).replace(/-/g, '');

	return (
		<tbody>
			<tr>
				<td>{post.nameOfDish}</td>
				<td className='hide-sm'>{post.cuisine}</td>
				<td className='hide-sm'>
					{todaysDate === moment(post.created).format('YYYYMMDD') ? (
						<p>Today</p>
					) : (
						<Moment format='DD/MM/YYYY'>{post.created}</Moment>
					)}
				</td>
				<td>
					<Ratings averageRating={post.averageRating} ratings={post.ratings} />
				</td>
				<td>
					<Link
						to={{
							pathname: `/posts/${post.postId}`,
							state: {
								fromAuthProfile: fromAuthProfile
							}
						}}
						className='btn btn-primary'
					>
						View
					</Link>
				</td>
			</tr>
		</tbody>
	);
};

export default ProfileItem;
