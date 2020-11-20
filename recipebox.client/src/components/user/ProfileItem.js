import React, { Fragment } from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import Moment from 'react-moment';
import moment from 'moment';

const ProfileItem = ({ post }) => {
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
					<Link to={`/posts/${post.postId}`} className='btn btn-primary'>
						View
					</Link>
					{/* <button className='btn btn-primary'>View</button> */}
					<button className='btn btn-danger'>Delete</button>
				</td>
			</tr>
		</tbody>
	);
};

export default ProfileItem;
