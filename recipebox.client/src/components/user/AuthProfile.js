import React, { Fragment, useEffect, useState } from 'react';
import { Redirect, withRouter, Prompt } from 'react-router-dom';
import Spinner from '../layout/Spinner';
import PropTypes from 'prop-types';
import { getPosts } from '../../actions/post';
import { getFavourites, deleteFavourite } from '../../actions/favourite';
import { addUserPhoto } from '../../actions/photo';
import { connect } from 'react-redux';
import ProfilePostItem from './ProfilePostItem';
import ProfileFavouriteItem from './ProfileFavouriteItem';
import PageNavigation from '../posts/PageNavigation';

const AuthProfile = ({
	getPosts,
	getFavourites,
	deleteFavourite,
	addUserPhoto,
	match,
	auth: { isAuthenticated, loading, user },
	post: { posts, post, loading: postsLoading },
	favourite: { favourites, favourite, favouritesLoading },
	profilePagination
}) => {
	const [ pageNumber, setPageNumber ] = useState(
		profilePagination.currentPage !== null ? profilePagination.currentPage : 1
	);
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ viewingPostType, setViewingPostType ] = useState(
		profilePagination.fromPosts !== null ? profilePagination.fromPosts : true
	);
	const [ file, setFile ] = useState(null);

	// Conditionally render either posts or favourites depending on which tab
	useEffect(
		() => {
			const orderBy = '';
			const searchParams = '';
			const userId = match.params.id.toString();

			if (viewingPostType) {
				getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
			} else {
				getFavourites({ userId, pageNumber, setLoadingPage, orderBy });
			}
		},
		[ getPosts, pageNumber, match.params.id, viewingPostType, profilePagination.loading ]
	);

	// Scroll to top of page upon mounting
	useEffect(() => {
		window.scrollTo(0, 0);
	}, []);

	// This effect will go back a page IF on favourites tab and the last favourite is deleted
	useEffect(
		() => {
			if (!favouritesLoading && !viewingPostType && favourites.length === 0) {
				setPageNumber(pageNumber - 1);
			}
		},
		[ favourites.length ]
	);

	const onChange = (e) => {
		setFile(e.target.files[0]);
	};

	const onSubmit = async (e) => {
		e.preventDefault();
		const formData = new FormData();
		formData.append('file', file);
		addUserPhoto(user.id, formData);
	};

	if (loading || loadingPage) {
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
					{/* <button> */}

					<div className='auth-profile-top p-2'>
						{user.userPhotos.length !== 0 ? (
							<img
								// onClick={() => <input type='file' id='input' />}
								className='round-img my-1'
								src={user.userPhotos[0].url}
								alt=''
							/>
						) : (
							<img
								// onClick={() => <input type='file' id='input' />}
								className='round-img my-1'
								src='https://kansai-resilience-forum.jp/wp-content/uploads/2019/02/IAFOR-Blank-Avatar-Image-1.jpg'
								alt='no avatar'
							/>
						)}
						<form onSubmit={onSubmit}>
							<input type='file' name='fileToUpload' onChange={onChange} />
							{file !== null && (
								<input type='submit' value='Save Profile Picture' className='btn btn-primary my-1' />
							)}
						</form>

						{/* <input type='submit' value='Upload Image' name='submit' /> */}
						{/* <small>Click icon to add or change your avatar</small> */}
					</div>
					{/* </button> */}
					<div className='dash-buttons'>
						<a href='edit-profile.html' className='btn btn-light'>
							<i className='fas fa-user-circle text-primary' /> Edit Profile
						</a>
					</div>

					{viewingPostType ? (
						<div className='tab'>
							<button className='active'>My Submissions</button>
							<button
								onClick={() => {
									setViewingPostType(false);
									setPageNumber(1);
								}}
								className='tablinks'
							>
								My Favourites
							</button>
						</div>
					) : (
						<div className='tab'>
							<button
								onClick={() => {
									setViewingPostType(true);
									setPageNumber(1);
								}}
								className='tablinks'
							>
								My Submissions
							</button>
							<button className='active'>My Favourites</button>
						</div>
					)}

					{/* On submissions tab  */}
					{viewingPostType &&
					posts.length !== 0 && (
						<Fragment>
							<table className='table'>
								<thead>
									<tr>
										<th>Name of Dish</th>
										<th className='hide-sm'>Cuisine</th>
										<th className='hide-sm'>Created</th>
										<th className='hide-sm'>Average Ratings</th>
									</tr>
								</thead>
								{posts.map((post) => (
									<ProfilePostItem key={post.postId} post={post} fromAuthProfile={true} />
								))}
							</table>
							<PageNavigation
								pagination={profilePagination}
								pageNumber={pageNumber}
								setPageNumber={setPageNumber}
							/>
						</Fragment>
					)}
					{viewingPostType && posts.length === 0 && <h1>You have not submitted any posts yet</h1>}

					{/* On favourites tab  */}
					{!viewingPostType &&
					favourites.length !== 0 && (
						<Fragment>
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
										fromAuthProfile={true}
									/>
								))}
							</table>
							<PageNavigation
								pagination={profilePagination}
								pageNumber={pageNumber}
								setPageNumber={setPageNumber}
							/>
						</Fragment>
					)}
					{!viewingPostType && favourites.length === 0 && <h1>You have not added any favourites yet</h1>}
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
	addUserPhoto: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	favourite: PropTypes.object.isRequired,
	profilePagination: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth,
	post: state.post,
	favourite: state.favourite,
	profilePagination: state.profilePagination,
	photo: state.photo
});

export default connect(mapStateToProps, { getPosts, getFavourites, deleteFavourite, addUserPhoto })(
	withRouter(AuthProfile)
);
