import React, { Fragment, useEffect, useState } from 'react';
import { Redirect, withRouter, Prompt } from 'react-router-dom';
import Spinner from '../layout/Spinner';
import PropTypes from 'prop-types';
import { getPosts } from '../../actions/post';
import { getFavourites, deleteFavourite } from '../../actions/favourite';
import { connect } from 'react-redux';
import ProfilePostItem from './ProfilePostItem';
import ProfileFavouriteItem from './ProfileFavouriteItem';

const AuthProfile = ({
	getPosts,
	getFavourites,
	deleteFavourite,
	match,
	auth: { isAuthenticated, loading, user },
	post: { posts, post, loading: postsLoading },
	favourite: { favourites, favourite, favouritesLoading }
}) => {
	const [ pageNumber, setPageNumber ] = useState(1);
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ formData, setFormData ] = useState({
		orderBy: '',
		searchParams: '',
		userId: match.params.id.toString()
	});
	const { orderBy, searchParams, userId } = formData;

	useEffect(
		() => {
			getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
		},
		[ getPosts, pageNumber, userId ]
	);

	useEffect(
		() => {
			getFavourites({ userId, pageNumber, setLoadingPage, orderBy });
		},
		[ getFavourites, pageNumber, userId ]
	);

	if (loading) {
		return <Spinner />;
	}

	return (
		<Fragment>
			{user.id.toString() !== match.params.id ? (
				<Redirect to='/posts' />
			) : (
				<Fragment>
					<h1 className='large text-primary'>Dashboard</h1>
					<p className='lead'>
						<i className='fas fa-user' /> Welcome {user.username.split(' ')[0]}
					</p>
					<div className='dash-buttons'>
						<a href='edit-profile.html' className='btn btn-light'>
							<i className='fas fa-user-circle text-primary' /> Edit Profile
						</a>
					</div>

					<h2 className='my-2'>My Submissions</h2>
					{posts.length === 0 ? (
						<h3>You have not yet submitted any posts</h3>
					) : (
						<table className='table'>
							<thead>
								<tr>
									<th>Name of Dish</th>
									<th className='hide-sm'>Cuisine</th>
									<th className='hide-sm'>Created</th>
									<th className='hide-sm'>Average Ratings</th>
								</tr>
							</thead>
							{posts.map((post) => <ProfilePostItem key={post.postId} post={post} />)}
						</table>
					)}

					<h2 className='my-2'>My Favorites</h2>
					{favourites.length === 0 ? (
						<h3>No Favourites Yet</h3>
					) : (
						<table className='table'>
							<thead>
								<tr>
									<th>Name Of Dish</th>
									<th className='hide-sm'>Cuisine</th>
									<th className='hide-sm'>Author</th>
									<th className='hide-sm'>Average Ratings</th>
								</tr>
							</thead>

							{favourites.map((favourite) => (
								<ProfileFavouriteItem
									key={favourite.postId}
									favourite={favourite}
									deleteFavourite={deleteFavourite}
									user={user}
								/>
							))}
						</table>
					)}
				</Fragment>
			)}
		</Fragment>
	);
};

AuthProfile.propTypes = {
	auth: PropTypes.object.isRequired,
	getPosts: PropTypes.func.isRequired,
	getFavourites: PropTypes.func.isRequired,
	deleteFavourite: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	favourite: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth,
	post: state.post,
	favourite: state.favourite
});

export default connect(mapStateToProps, { getPosts, getFavourites, deleteFavourite })(withRouter(AuthProfile));
