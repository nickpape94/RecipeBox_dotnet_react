import React, { Fragment, useEffect, useState } from 'react';
import { Redirect, withRouter, Prompt } from 'react-router-dom';
import Spinner from '../layout/Spinner';
import PropTypes from 'prop-types';
import { getPosts } from '../../actions/post';
import { getFavourites, deleteFavourite } from '../../actions/favourite';
import { addUserPhoto, deleteUserPhoto } from '../../actions/photo';
import { addUpdateAboutSection } from '../../actions/user';
import { connect } from 'react-redux';
import ProfilePostItem from './ProfilePostItem';
import ProfileFavouriteItem from './ProfileFavouriteItem';
import PageNavigation from '../posts/PageNavigation';

const AuthProfile = ({
	getPosts,
	getFavourites,
	deleteFavourite,
	addUserPhoto,
	deleteUserPhoto,
	match,
	auth: { loading, user, error },
	post: { posts, post, loading: postsLoading },
	favourite: { favourites, favourite, favouritesLoading },
	addUpdateAboutSection,
	profilePagination
}) => {
	const [ file, setFile ] = useState(null);
	const [ showAboutSection, setShowAboutSection ] = useState(false);
	const [ about, setAbout ] = useState('');
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ viewingPostType, setViewingPostType ] = useState(
		profilePagination.fromPosts !== null ? profilePagination.fromPosts : true
	);
	const [ pageNumber, setPageNumber ] = useState(
		profilePagination.currentPage !== null ? profilePagination.currentPage : 1
	);

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

	// Load about section
	useEffect(
		() => {
			setAbout(user && user.about !== null ? user.about : '');
		},
		[ loading ]
	);

	const onChange = (e) => {
		if (e.target.name === 'fileToUpload') {
			setFile(e.target.files[0]);
		}
		if (e.target.name === 'about') {
			setAbout(e.target.value);
		}
	};

	const onProfilePicSubmit = async (e) => {
		e.preventDefault();
		const formData = new FormData();
		formData.append('file', file);
		addUserPhoto(user.id, formData);
	};

	const onAboutSectionSubmit = async (e) => {
		e.preventDefault();
		addUpdateAboutSection(user.id, about);
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
							<Fragment>
								<img
									// onClick={() => <input type='file' id='input' />}
									className='round-img my-1'
									src={user.userPhotos[0].url}
									alt=''
								/>
								<button
									className='btn btn-danger'
									onClick={() => deleteUserPhoto(user.id, user.userPhotos[0].userPhotoId)}
								>
									<i className='fas fa-trash-alt'> Delete Profile Picture</i>
								</button>
							</Fragment>
						) : (
							<img
								// onClick={() => <input type='file' id='input' />}
								className='round-img my-1'
								src='https://kansai-resilience-forum.jp/wp-content/uploads/2019/02/IAFOR-Blank-Avatar-Image-1.jpg'
								alt='no avatar'
							/>
						)}
						<form onSubmit={(e) => onProfilePicSubmit(e)}>
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
						<button onClick={() => setShowAboutSection(true)} className='btn btn-light'>
							<i className='fas fa-user-circle text-primary' /> Edit bio
						</button>
					</div>
					{showAboutSection && (
						<form onSubmit={(e) => onAboutSectionSubmit(e)}>
							<div className='form-group'>
								<div className='form-group'>
									<textarea
										cols='30'
										rows='5'
										placeholder='About'
										// type='description'
										name='about'
										value={about}
										onChange={(e) => onChange(e)}
									/>
								</div>
								<div className='form-group'>
									<input type='submit' className='btn btn-primary' value='Save' />
								</div>
							</div>
						</form>
					)}
					{error !== null && <small>{error}</small>}

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
	deleteUserPhoto: PropTypes.func.isRequired,
	addUpdateAboutSection: PropTypes.func.isRequired,
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

export default connect(mapStateToProps, {
	getPosts,
	getFavourites,
	deleteFavourite,
	addUserPhoto,
	deleteUserPhoto,
	addUpdateAboutSection
})(withRouter(AuthProfile));
